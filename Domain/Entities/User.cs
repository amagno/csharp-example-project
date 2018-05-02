using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Core;

namespace Domain.Entities
{
    public class User : Entity
    {
        

        public string Name { get; }
        public string Email { get; }
        
        private readonly IList<Project> _projects = new List<Project>();
        public virtual IReadOnlyCollection<Project> Projects => _projects.ToArray();
        
        public User(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public void AddProject(DateTime startDate, DateTime finishDate, string title, string description)
        {
            var verifyStartDate = _projects
                .Where(p => (startDate >= p.StartDate) && (startDate <= p.FinishDate))
                .ToList();
            var verifyFinishDate = _projects
                .Where(p => (finishDate <= p.FinishDate) || (startDate <= p.StartDate && finishDate > p.FinishDate))
                .ToList();
            
            
            if (verifyStartDate.Count > 0 || verifyFinishDate.Count > 0) throw new Exception("Date are invalid");
            
            _projects.Add(new Project(this, startDate, finishDate, title, description));
        }
    }
}