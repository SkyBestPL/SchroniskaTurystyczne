using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchroniskaTurystyczne.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchroniskaTurystyczne.ViewModels
{
    public class ShelterCreateViewModel
    {
        public ShelterCreateViewModel()
        {
            Categories = new List<SelectListItem>();
            RoomTypes = new List<SelectListItem>();
            Tags = new List<Tag>();
            Facilities = new List<Facility>();
            Rooms = new List<RoomViewModel>();
            SelectedTags = new List<int>();
        }

        [Required (ErrorMessage = "Nazwa schroniska jest wymagana.")]
        [MaxLength(50, ErrorMessage = "Nazwa schroniska może mieć maksymalnie 50 znaków.")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Opis schroniska jest wymagany.")]
        [MaxLength(1000, ErrorMessage = "Opis schroniska może mieć maksymalnie 1000 znaków.")]
        public string Description { get; set; }
        [Required (ErrorMessage = "Kraj jest wymagany.")]
        [MaxLength(100, ErrorMessage = "Kraj może mieć maksymalnie 100 znaków.")]
        public string Country { get; set; }
        [Required (ErrorMessage = "Miasto jest wymagane.")]
        [MaxLength(100, ErrorMessage = "Miasto może mieć maksymalnie 100 znaków.")]
        public string City { get; set; }
        [MaxLength(200, ErrorMessage = "Ulica może mieć maksymalnie 200 znaków.")]
        public string? Street { get; set; }
        [MaxLength(50, ErrorMessage = "Numer ulicy może mieć maksymalnie 50 znaków.")]
        public string? StreetNumber { get; set; }
        [Required (ErrorMessage = "Kod pocztowy jest wymagany.")]
        [MaxLength(20, ErrorMessage = "Kod pocztowy może mieć maksymalnie 20 znaków.")]
        public string ZipCode { get; set; }
        [Required (ErrorMessage = "Lokalizacja jest wymagana.")]
        public string LocationLon { get; set; }
        [Required (ErrorMessage = "Lokalizacja jest wymagana.")]
        public string LocationLat { get; set; }
        [Required (ErrorMessage = "Kategoria jest wymagana.")]
        public int IdCategory { get; set; }
        [ModelBinder(BinderType = typeof(JsonModelBinder))]
        public List<int> SelectedTags { get; set; } = new List<int>();
        [NotMapped]
        public string? SelectedTagsString { get; set; }

        public List<RoomViewModel> Rooms { get; set; } = new List<RoomViewModel>();
        public List<IFormFile>? Photos { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? RoomTypes { get; set; }
        public IEnumerable<Tag>? Tags { get; set; }
        public IEnumerable<Facility>? Facilities { get; set; }
    }

    public class RoomViewModel
    {
        [Required(ErrorMessage = "Wybierz typ pokoju")]
        public int? IdType { get; set; }
        [Required(ErrorMessage = "Nazwa pokoju jest wymagana")]
        [MaxLength(100, ErrorMessage = "Nazwa pokoju może mieć maksymalnie 100 znaków.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Cena pokoju jest wymagana.")]
        [Range(0.00, 10000, ErrorMessage = "Cena musi być pomiędzy 0.00 a 10 000.")]
        public decimal PricePerNight { get; set; }
        [Required(ErrorMessage = "Pojemność pokoju jest wymagana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pojemność musi wynosić co najmniej 1.")]
        public int Capacity { get; set; }
        [ModelBinder(BinderType = typeof(JsonModelBinder))]
        public List<int> SelectedFacilities { get; set; } = new List<int>();
        [NotMapped]
        public string? SelectedFacilitiesString { get; set; }
        public bool IsActive { get; set; }
        public List<IFormFile>? RoomPhotos { get; set; }
    }

    public class JsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue;
                var model = string.IsNullOrEmpty(value)
                    ? new List<int>()
                    : JsonConvert.DeserializeObject<List<int>>(value);

                bindingContext.Result = ModelBindingResult.Success(model ?? new List<int>());
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(modelName, $"Nieprawidłowy format danych: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
