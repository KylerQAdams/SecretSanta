using SecretSanta.Extensions;
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
        /// <summary>Maximum number of participants allowed to process at one time.</summary>
        /// <remarks>
        /// A maximum is used for two reasons:
        /// 1) Because everything is stored and operated on in memory, extremely large values can give OutOfMemory exceptions depending on hardware.
        /// 2) Large amounts of data can also take awhile to process.  Solve solution could be optimized/improved but was not for the sake of time.
        /// </remarks>
        public static readonly int MAX_INPUT_AMOUNT = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxParticipants"]);


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
        /// <summary>
        /// Validates input and sets up data to call <see cref="ExecuteListGeneration(IList{Participant})"/> for output.
        /// Returns matched Giver/Recipient pairs for every individual.
        /// </summary>
        public List<SantaResult> GenerateGiverRecipients(GroupOfParticipants[] groups)
        {
            // Validation (1/2)
            if (groups == null || groups.Length <= 1)
                throw new ArgumentException("There are not enough people to generate a Secret Santa List.");

            #region Setup
            // Remove empities, null, and trim.
            foreach (var g in groups)
                g.Names = g.Names.TrimAndRemoveEmpities().ToArray();

            // Orders by group size
            var orderedGroups = groups.OrderByDescending(g => g.Names.Length).ToArray();
            
            // Flattens into one list and assigns group numbers for easier manipulation
            var participants = new List<Participant>();
            for (int i = 0; i < orderedGroups.Length; i++)
            {
                foreach (String name in orderedGroups[i].Names)
                {
                    participants.Add(new Participant(i, name?.Trim()));
                }
            }
            #endregion

            // Validation (2/2)
            if (participants.Count <= 1)
                throw new ArgumentException("There are not enough people to generate a Secret Santa List.");

            if (participants.Count > MAX_INPUT_AMOUNT)
                throw new ArgumentException($"The number of participants ({participants.Count}) exceeds the configured maximum ({MAX_INPUT_AMOUNT}).");

            if (orderedGroups[0].Names.Length * 2 > participants.Count)
                throw new ArgumentException($"The biggest group is more than half the size of the total people.  Please adjust the groups or add more people.");

            var uniqueTest = new HashSet<String>();
            foreach (var p in participants)
                if (!uniqueTest.Add(p.Name))
                    throw new ArgumentException($"Duplicate participants with the name \"{p.Name}\" detected.  Please make the names unique to avoid confusion.");

            // Execution
            return ExecuteListGeneration(participants);
        }

        /// <summary>
        /// Assigns each participant a unique recipient not within their group.
        /// </summary>
        public List<SantaResult> ExecuteListGeneration(List<Participant> participants)
        {
            // If there exists participants without a recipient, go through and attempt to assign an recipient.
            while (participants.Exists(p => p.Recipient == null))
            {
                foreach(var participant in participants.Where(p => p.Recipient == null))
                    AssignParticipant(participant, participants);
            }

            var returnValue = new List<SantaResult>();
            foreach (var participant in participants)
                returnValue.Add( new SantaResult() { Giver = HttpUtility.HtmlEncode(participant.Name),
                                                     Recipient = HttpUtility.HtmlEncode(participant.Recipient.Name)
                                                    });

            return returnValue;
        }

        /// <summary>
        /// Attempts to match the unassigned with a participant who is not receiving a gift and is a valid choice.
        /// On failure, the unassigned steals the recipient from a random participant in another group's recipient.
        /// This logic could be optimized, but is kept as is for the sake of time.
        /// </summary>
        /// <param name="unassigned"></param>
        /// <param name="participants"></param>
        private void AssignParticipant(Participant unassigned, List<Participant> participants)
        {
            // Find participants who do not currently have a gift-giver outside the unassigned group.
            var assignableParticipants = participants.Where(p => p.GroupID != unassigned.GroupID
                                                      && p.Giver == null).ToList();

            if (assignableParticipants.Count > 0)
            {
                var recipient = assignableParticipants.GetRandom();
                unassigned.Recipient = recipient;
            }
            else
            {
                // Find participants we can steal the recipient from without any further swapping needed.
                var swappableParticipants = participants.Where(p => p.GroupID != unassigned.GroupID
                                                   && p.Recipient != null
                                                   && p.Recipient.GroupID != unassigned.GroupID
                                                   && participants.Exists(p2 => p2.GroupID != p.GroupID && p2.Giver == null) )
                                                  .ToList();

                // If none are available, just find any participants we can steal the recipient from.
                if (swappableParticipants.Count == 0)
                    swappableParticipants = participants.Where(p => p.GroupID != unassigned.GroupID
                                                       && p.Recipient != null
                                                       && p.Recipient.GroupID != unassigned.GroupID)
                                                      .ToList();

                var victum = swappableParticipants.GetRandom();
                unassigned.Recipient = victum.Recipient;
                victum.Recipient = null;
            }
        }
        #endregion
    }


}
