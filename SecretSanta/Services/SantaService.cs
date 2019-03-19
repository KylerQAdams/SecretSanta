using SecretSanta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecretSanta.Services.Interfaces;

namespace SecretSanta.Services
{
    /// <summary>
    /// Service used to generate output.
    /// </summary>
    public class SantaService : ISantaService
    {

        #region Constructor
        /// <summary>
        /// Gets the Santaservice
        /// </summary>
        public static ISantaService CreateService()
        {
            return new SantaService();
        }
        /// <summary>
        /// Use <see cref="CreateService"/> to get an instance.
        /// </summary>
        private SantaService()
        {
           
        }
        #endregion


        #region Participant matching
        public List<SantaResult> GenerateGiverRecipients(GroupOfParticipants[] groups)
        {
        }

        #endregion
    }


}
