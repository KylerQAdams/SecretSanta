using SecretSanta.Models;
using SecretSanta.Services.Interfaces;
using System;
using System.Net;
using System.Web.Mvc;
using SecretSanta.Services;
using System.Web;

namespace SecretSanta.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Assigns gift recipients to each named participant while not allowing
        /// participants to match to others in their same group.
        /// </summary>
        /// <param name="groups">Array of groups that contain the participant names</param>
        /// <returns>Array of paired string values for Giver and Recipient in JSON</returns>
        [HttpPost]
        public ActionResult GetSantasList(GroupOfParticipants[] groups)
        {
            try
            {
                ISantaService service = SantaService.CreateService();
                var result = service.GenerateGiverRecipients(groups);
                return Json(result);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, HttpUtility.HtmlEncode(e.Message));
            }
        }

    }
}