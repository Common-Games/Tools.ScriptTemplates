using System;
using System.Collections.Generic;
using NUnit.Framework;

using CGTK.Tools.CustomScriptTemplates;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public sealed class TestScript
    {
        //TODO: Write actual Unit Tests PLZ
        //Test paths with wrong symbols, looping paths, those kind of things.
    
        // A Test behaves as an ordinary method
        [Test]
        public void GetAllTemplates()
        {
            //IEnumerable<(String folders, String name, String path)> __templates = Templates.Gather();

            /*
            foreach ((String __folders, String __name, String __path) in __templates)
            {
                Debug.Log(message: $"folders: {__folders} name: {__name}, path: {__path}");
            }
            */
        }
    }
}
