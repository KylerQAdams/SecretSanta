using SecretSanta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Services.Interfaces
{
    public interface ISantaService
    {
        /// <summary>
        /// Returns matched Giver/Recipient pairs for every provided individual, making sure no individual is matched within its group.
        /// </summary>
        List<SantaResult> GenerateGiverRecipients(GroupOfParticipants[] groups);

    }
}
