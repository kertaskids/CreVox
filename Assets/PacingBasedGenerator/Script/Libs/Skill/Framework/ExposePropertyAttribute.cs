using UnityEngine;
using System;


namespace Skill.Framework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExposePropertyAttribute : Attribute
    {
        public int Order { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ExposePropertyAttribute(int order, string name, string description = null)
        {
            this.Order = order;
            this.Name = name;
            this.Description = description;
            this.TextAreaHeight = 18;
        }


        public bool RightToLeft { get; set; }
        public bool PasteEnable { get; set; }
        public bool TextArea { get; set; }
        public float TextAreaHeight { get; set; }

        public bool Left { get; set; }
        public bool Delayed { get; set; }
    }
}