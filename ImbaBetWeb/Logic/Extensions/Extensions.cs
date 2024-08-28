using ImbaBetWeb.ViewModels.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImbaBetWeb.Logic.Extensions
{
    public static class Extensions
    {
        public static void SetInfoAlert(this Controller controller, string info)
        {
            UpdateAlerts(controller, (dto) => { dto.Infos.Add(info); });
        }

        public static void SetErrorAlert(this Controller controller, string info)
        {
            UpdateAlerts(controller, (dto) => { dto.Errors.Add(info); });
        }

        public static void SetSuccessAlert(this Controller controller, string info)
        {
            UpdateAlerts(controller, (dto) => { dto.Successes.Add(info); });
        }

        private static void UpdateAlerts(Controller controller, Action<AlertsDTO> action)
        {
            var temp = controller.TempData["Alerts"];
            var dto = temp is string str ? AlertsDTO.FromJson(str) : new AlertsDTO();

            action(dto);

            controller.TempData["Alerts"] = dto.ToJson();
        }

        /// <summary>
        /// Validates a given model of the page
        /// </summary>
        public static bool IsValid<T>(this PageModel pageModel, T inputModel)
        {
            var property = pageModel.GetType().GetProperties().Where(x => x.PropertyType == inputModel.GetType()).FirstOrDefault();

            var hasErrors = pageModel.ModelState.Values
                .Where(value => value.GetType().GetProperty("Key").GetValue(value).ToString().Contains(property.Name))
                .Any(value => value.Errors.Any());

            return !hasErrors;
        }
    }
}
