using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.IO;

namespace Skill.Editor.CodeGeneration
{
    /// <summary>
    /// Generate C# code for SaveData
    /// </summary>
    class SerializedDataClass : ClassDataClass
    {
        private SerializedData _SaveData;        

        /// <summary>
        /// Create an instance of SaveData
        /// </summary>
        /// <param name="saveData">SaveData model</param>
        public SerializedDataClass(SerializedData saveData)
            : base(saveData)
        {

            if (saveData.Classes == null)
                saveData.Classes = new ClassData[0];


            this._SaveData = saveData;
            IsPartial = saveData.IsPartial;
            CreateClasses();
        }

        /// <summary>
        /// Create internal classes
        /// </summary>
        private void CreateClasses()
        {
            foreach (var item in _SaveData.Classes)
            {
                ClassDataClass saveClass = new ClassDataClass(item);                
                Add(saveClass);
            }
        }        
    }
}
