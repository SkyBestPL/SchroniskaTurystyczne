using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;

namespace SchroniskaTurystyczne.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public MessageController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> IsUserAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var roles = await _userManager.GetRolesAsync(user);

            return roles.Contains("Admin");
        }

        public async Task<IActionResult> Index(int? shelterId, string? userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var currentUserShelter = await _context.Shelters
                .FirstOrDefaultAsync(s => s.IdExhibitor == currentUser.Id);

            var viewModel = new MessageViewModel
            {
                CurrentUserId = currentUser.Id,
                CurrentUserName = currentUserShelter != null
                    ? currentUserShelter.Name 
                    : $"{currentUser.FirstName} {currentUser.LastName}",
                Conversations = new List<ConversationViewModel>(),
                InitialShelterId = shelterId
            };

            var messages = await _context.Messages
                .Where(m => m.IdSender == currentUser.Id || m.IdReceiver == currentUser.Id)
                .OrderBy(m => m.Date)
                .ToListAsync();

            if (shelterId.HasValue)
            {
                var shelter = await _context.Shelters
                    .FirstOrDefaultAsync(s => s.Id == shelterId.Value);

                bool isAdmin = await IsUserAdmin(userId);

                if (shelter != null)
                {
                    var shelterOwner = await _context.Users.FindAsync(shelter.IdExhibitor);

                    viewModel.Receiver = new ReceiverViewModel
                    {
                        Id = shelter.IdExhibitor,
                        DisplayName = shelter.Name
                    };

                    var shelterMessages = messages
                        .Where(m => m.IdReceiver == shelter.IdExhibitor || m.IdSender == shelter.IdExhibitor)
                        .ToList();

                    viewModel.CurrentConversation = new ConversationViewModel
                    {
                        RelatedShelter = new ShelterInfoViewModel
                        {
                            Id = shelter.Id,
                            Name = shelter.Name
                        },
                        OtherUserId = shelter.IdExhibitor,
                        OtherUserName = shelter.Name,
                        IsExhibitor = true,
                        IsAdmin = isAdmin,
                        IsNewConversation = !shelterMessages.Any(),
                        Messages = shelterMessages.Select(m => new MessageInfoViewModel
                        {
                            Contents = m.Contents,
                            SentAt = m.Date,
                            IsSentByCurrentUser = m.IdSender == currentUser.Id
                        }).ToList()
                    };
                }
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                var otherUser = await _context.Users.FindAsync(userId);
                var relatedShelter = await _context.Shelters
                    .FirstOrDefaultAsync(s => s.IdExhibitor == currentUser.Id);

                bool isAdmin = await IsUserAdmin(userId);

                if (otherUser != null)
                {
                    viewModel.Receiver = new ReceiverViewModel
                    {
                        Id = otherUser.Id,
                        DisplayName = otherUser.FirstName + " " + otherUser.LastName
                    };

                    var userMessages = messages
                        .Where(m => m.IdReceiver == otherUser.Id || m.IdSender == otherUser.Id)
                        .ToList();

                    viewModel.CurrentConversation = new ConversationViewModel
                    {
                        RelatedShelter = relatedShelter != null ? new ShelterInfoViewModel
                        {
                            Id = relatedShelter.Id,
                            Name = relatedShelter.Name
                        } : null,
                        OtherUserId = otherUser.Id,
                        OtherUserName = otherUser.FirstName + " " + otherUser.LastName,
                        IsNewConversation = !userMessages.Any(),
                        IsExhibitor = false,
                        IsAdmin = isAdmin,
                        Messages = userMessages.Select(m => new MessageInfoViewModel
                        {
                            Contents = m.Contents,
                            SentAt = m.Date,
                            IsSentByCurrentUser = m.IdSender == currentUser.Id
                        }).ToList()
                    };
                }
            }

            var conversationUserIds = messages
            .Select(m => m.IdSender == currentUser.Id ? m.IdReceiver : m.IdSender)
            .Distinct()
            .ToList();

            foreach (var otherUserId in conversationUserIds)
            {
                var otherUser = await _context.Users.FindAsync(otherUserId);
                var relatedShelter = await _context.Shelters
                    .FirstOrDefaultAsync(s => s.IdExhibitor == otherUserId);

                var conversationViewModel = CreateConversationViewModel(currentUser, otherUser, relatedShelter, messages);

                if (conversationViewModel != null)
                {
                    viewModel.Conversations.Add(conversationViewModel);
                }
            }

            viewModel.Conversations = viewModel.Conversations
                .OrderByDescending(c => c.Messages.LastOrDefault()?.SentAt)
                .ToList();

            foreach (var conversation in viewModel.Conversations)
            {
                conversation.UnreadMessagesCount = messages
                    .Count(m => m.IdSender == conversation.OtherUserId &&
                                m.IdReceiver == currentUser.Id &&
                                !m.IsRead);
            }

            if (viewModel.CurrentConversation != null)
            {
                var unreadMessages = await _context.Messages
                    .Where(m => m.IdSender == viewModel.CurrentConversation.OtherUserId &&
                                m.IdReceiver == currentUser.Id &&
                                !m.IsRead)
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }

            return View(viewModel);
        }

        private ConversationViewModel CreateConversationViewModel(AppUser currentUser, AppUser otherUser, Shelter relatedShelter, List<Message> messages)
        {
            if (currentUser == null || otherUser == null)
            {
                return null;
            }

            var conversationMessages = messages
                .Where(m =>
                    (m.IdSender == currentUser.Id && m.IdReceiver == otherUser.Id) ||
                    (m.IdSender == otherUser.Id && m.IdReceiver == currentUser.Id)
                )
                .OrderBy(m => m.Date)
                .Select(m => new MessageInfoViewModel
                {
                    Id = m.Id,
                    Contents = m.Contents,
                    SentAt = m.Date,
                    IsSentByCurrentUser = m.IdSender == currentUser.Id
                })
                .ToList();

            return new ConversationViewModel
            {
                OtherUserId = otherUser.Id,
                OtherUserName = relatedShelter != null ? relatedShelter.Name : $"{otherUser.FirstName} {otherUser.LastName}",
                RelatedShelter = relatedShelter != null ? new ShelterInfoViewModel
                {
                    Id = relatedShelter.Id,
                    Name = relatedShelter.Name
                } : null,
                Messages = conversationMessages,
                IsNewConversation = false
            };
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewMessageContent))
            {
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (string.IsNullOrEmpty(model.Receiver?.Id))
            {
                return BadRequest("Nie można ustalić odbiorcy wiadomości.");
            }

            var newMessage = new Message
            {
                IdSender = currentUser.Id,
                IdReceiver = model.Receiver.Id,
                Contents = model.NewMessageContent,
                Date = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            if (model.InitialShelterId != null)
            {
                return RedirectToAction(nameof(Index), new { shelterId = model.InitialShelterId });
            }
            else
            {
                return RedirectToAction(nameof(Index), new { userId = model.Receiver.Id });
            }
        }

        public async Task<IActionResult> MarkMessagesAsRead(string otherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var unreadMessages = await _context.Messages
                .Where(m => m.IdSender == otherUserId && m.IdReceiver == currentUser.Id && !m.IsRead).ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<bool> HasUnreadMessages()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return false;
            }

            return await _context.Messages
                .AnyAsync(m => m.IdReceiver == currentUser.Id && !m.IsRead);
        }

        public async Task<IActionResult> GetAdminContact()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
                return NotFound("Nie znaleziono roli administratora.");

            var adminUser = await _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Admin"));

            if (adminUser == null)
                return NotFound("Nie znaleziono administratora.");

            return RedirectToAction("Index", new { userId = adminUser.Id });
        }
    }
}
