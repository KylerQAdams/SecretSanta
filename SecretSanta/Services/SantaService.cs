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
        /// <summary>
        /// Maximum number of participants allowed to process at one time.
        /// </summary>
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
        /// Returns a list of <seealso cref="SantaResult"/>s for every provided participant,
        /// making sure no participant is matched to another within its own group.
        /// </summary>
        /// <exception cref="ArgumentException">Input is unable to generate a list.</exception>
        public List<SantaResult> CreateSantaList(GroupOfParticipants[] groups)
        {
            var participants = ValidateAndSetup(groups);
            return GenerateList(participants);
        }


        /// <summary>
        /// Validates input and returns a flat, ordered, list of participants for later proccessing.
        /// </summary>
        /// <exception cref="ArgumentException">Input is unable to generate a list.</exception>
        public List<Participant> ValidateAndSetup(GroupOfParticipants[] groups)
        {
            if (groups == null || groups.Length <= 1)
                throw new ArgumentException("There are not enough people to generate a Secret Santa List.");


            #region Setup
            foreach (var g in groups)
                g.Names = g.Names.TrimAndRemoveEmpities().ToArray();
            var orderedGroups = groups.OrderByDescending(g => g.Names.Length).ToArray();

            // flatten and rip into new object.
            var participants = new List<Participant>();
            for (int i = 0; i < orderedGroups.Length; i++)
            {
                foreach (String name in orderedGroups[i].Names)
                {
                    participants.Add(new Participant(i, name?.Trim()));
                }
            }
            #endregion


            if (participants.Count <= 1)
                throw new ArgumentException("There are not enough people to generate a Secret Santa List.");

            if (participants.Count > MAX_INPUT_AMOUNT)
                throw new ArgumentException($"The number of participants ({participants.Count}) exceeds the configured maximum ({MAX_INPUT_AMOUNT}).");

            if (orderedGroups[0].Names.Length * 2 > participants.Count)
                throw new ArgumentException($"The biggest group is more than half the size of the total people.  Please adjust the groups or add more people.");

            var (AreUnique, Duplicant) = participants.Select(p => p.Name).AreAllUnique();
            if (!AreUnique)
                throw new ArgumentException($"Duplicant participants with the name [{Duplicant}] detected.  Please make the names unique to avoid confusion.");

            return participants;
        }


        /// <summary>
        /// Assigns each participant a random unique recipient not within their group.
        /// </summary>
        public List<SantaResult> GenerateList(List<Participant> participants)
        {

            // The assignment method will sometimes steal the assignments of already-assigned participants (by design).
            // Rather than adding to the call stack and recursively calling AssignParticipants for these individuals,
            // opted to add the follow while loop & where condition.
            while (participants.Exists(p => p.Recipient == null))
            {
                foreach(var participant in participants.Where(p => p.Recipient == null))
                    AssignParticipant(participant, participants);
            }


            // Names are HtmlEncoded since they are displayed directly in the UI.
            var returnValue = new List<SantaResult>();
            foreach (var participant in participants)
            {
                returnValue.Add(new SantaResult()
                {
                    Giver = HttpUtility.HtmlEncode(participant.Name),
                    Recipient = HttpUtility.HtmlEncode(participant.Recipient.Name)
                });
            }

            return returnValue;
        }

        /// <summary>
        /// Attempts to match the unassigned with a participant who is not receiving a gift and is a valid choice.
        /// On failure, the unassigned steals the recipient from a random participant in another group's recipient.
        /// </summary>
        /// <param name="unassigned"></param>
        /// <param name="participants"></param>
        private void AssignParticipant(Participant unassigned, List<Participant> participants)
        {
            // Find participants who are not currently receiving a gift and are also outside the unassigned group.
            var assignableParticipants = participants.Where(p => p.GroupID != unassigned.GroupID
                                                                && p.Giver == null).ToList();


            // If at least one exists, randomly select one and done!
            if (assignableParticipants.Count > 0)
            {
                var recipient = assignableParticipants.GetRandom();
                unassigned.Recipient = recipient;
            }
            else // Otherwise, try and steal from an existing recipient.
            {
                // Best case is if the existing recipient we steal from can be assigned to another recipient without needing to swap again.
                var swappableParticipants = participants.Where(p => p.GroupID != unassigned.GroupID
                                                                    && p.Recipient != null
                                                                    && p.Recipient.GroupID != unassigned.GroupID
                                                                    && participants.Exists(p2 => p2.GroupID != p.GroupID && p2.Giver == null) )
                                                                    .ToList();

                // Worst case is we will likely need to swap again.
                if (swappableParticipants.Count == 0)
                {
                    swappableParticipants = participants.Where(p => p.GroupID != unassigned.GroupID
                                                                    && p.Recipient != null
                                                                    && p.Recipient.GroupID != unassigned.GroupID)
                                                                    .ToList();
                }

                var victum = swappableParticipants.GetRandom();
                unassigned.Recipient = victum.Recipient;
                victum.Recipient = null;
            }
        }
        #endregion
    }


}
