using ImbaBetWeb.ViewModels.DTO;
using Microsoft.AspNetCore.Mvc;

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
    }
}
