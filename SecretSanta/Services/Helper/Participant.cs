using System;

namespace SecretSanta.Services
{

    public class Participant
    {
        private Participant _recipient;
        private Participant _giver;

        /// <summary>
        /// People with the same GroupID should not be matched.
        /// </summary>
        public int GroupID { get; set; }
        /// <summary> 
        /// Name of the Participant.
        /// </summary>
        public String Name { get; set; }
        /// <summary> 
        /// Who this person needs to give a gift to.  
        /// </summary>
        public Participant Recipient
        {
            get
            {
                return _recipient;
            }
            set
            {
                _recipient = value;
                if (value != null)
                    value._giver = this;
            }
        }
        /// <summary> 
        /// Who is currently giving a gift to this person.
        /// When a participant is assigned a recipient, the recipient's Giver is updated automatically.
        /// </summary>
        public Participant Giver { get { return _giver; } }

        /// <param name="groupId">ID for the group this participant is in.  Participants that share this value should not be matched with each other.</param>
        /// <param name="name">Name of the participant</param>
        public Participant(int groupId, String name)
        {
            this.GroupID = groupId;
            this.Name = name;
        }

    }
}