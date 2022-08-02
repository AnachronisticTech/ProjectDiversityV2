using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;

/// <summary>
///     [What does this TODOV2 do]
/// </summary>
public sealed class TODOV2 : MonoBehaviour
{
    public class Client
    {
        private readonly string _name;
        private readonly int _age;

        public Client(string name = "Name", int age = 0)
        {
            _name = name;
            _age = age;
        }

        public string GetName { get { return _name; } }
        public int GetAge { get { return _age; } }
    }

    private readonly object[] clients = { "Panikkos",
                                          "Kostas" };
    private readonly object[] clients2 = { new Client("Panikkos", 25), 
                                           new Client("Kostas", 22) };

    private void Start()
    {
        Console.Log(clients2);
    }
}
