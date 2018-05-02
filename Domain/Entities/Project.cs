using System;

namespace Domain.Entities
{
    public class Project
    {
        public bool Completed { get; }
        public virtual User Owner { get; }
        public DateTime StartDate { get; }
        public DateTime FinishDate { get; }
        public string Title { get; }
        public string Descriptiion { get; }

        public Project(
            User owner, 
            DateTime startDate, 
            DateTime finishDate, 
            string title, 
            string descriptiion, 
            bool completed = false
        )
        {
            Completed = completed;
            Owner = owner;
            StartDate = startDate;
            FinishDate = finishDate;
            Title = title;
            Descriptiion = descriptiion;
        }
    }
}