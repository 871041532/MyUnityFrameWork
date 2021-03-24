using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace EditorLearn
{
    [Serializable]
    public class MyClass
    {
        // Despite the OdinSerialize attribute, this field is not serialized.
        [OdinSerialize]
        public Dictionary<int, string> SecondDictionary;
    }
    
    public class ExampleScript : SerializedMonoBehaviour
    {
        // Unity will not serialize. Serialized by Odin.
        public Dictionary<int, string> FirstDictionary;

        // Unity will serialize. NOT serialized by Odin.
        public MyClass MyReference;
    }
    
    public class SerializedTest
    {
        
    }
}