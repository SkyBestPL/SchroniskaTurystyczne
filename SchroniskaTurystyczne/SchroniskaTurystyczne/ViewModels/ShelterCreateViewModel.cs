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
        public string Name { get; set; }
        [Required (ErrorMessage = "Opis schroniska jest wymagany.")]
        public string Description { get; set; }
        [Required (ErrorMessage = "Kraj jest wymagany.")]
        public string Country { get; set; }
        [Required (ErrorMessage = "Miasto jest wymagane.")]
        public string City { get; set; }
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        [Required (ErrorMessage = "Kod pocztowy jest wymagany.")]
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

        [Required(ErrorMessage = "Dodaj przynajmniej jeden pokój")]
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
        public string Name { get; set; }
        [Required(ErrorMessage = "Cena jest wymagana.")]
        public decimal PricePerNight { get; set; }
        [Required(ErrorMessage = "Pojemność jest wymagana.")]
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
