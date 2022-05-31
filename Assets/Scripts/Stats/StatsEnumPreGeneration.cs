using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text.RegularExpressions;

/// <summary>
///     Generates the Stat enum types when there is a change to the StatRepo script at precompile state.
/// </summary>
/// <seealso cref="StatRepo"/>
/// <seealso cref="StatTypes"/>
public class StatsEnumPreGeneration : AssetPostprocessor
{
    protected void OnPreprocessAsset()
    {
        List<string> statsList = new List<string>();

        // get the StatRepo.cs file in the current directory
        if (assetPath.Contains("StatRepo.cs", StringComparison.OrdinalIgnoreCase))
        {
            // read the StatRepo file line by line and store it in an array
            string[] statsRepoFile = File.ReadAllLines(assetPath);

            // pattern for: =[as many spaces here]"[word]" 
            string variableDeclarationPattern = @"=\s+\W(?<stat>\w+)\W";

            // check all lines of the array
            for (int i = 0; i < statsRepoFile.Length; i++)
            {
                // compare line with pattern and find matches
                Match match = Regex.Match(statsRepoFile[i], variableDeclarationPattern);

                if (match.Success)
                {
                    // add matches to the list of stats
                    statsList.Add(match.Groups["stat"].Value);
                }
            }
        }

        // contruct the new script with the new enum states
        string output = "// Auto-generated script from StatsEnumPreGeneration.cs\n// Do NOT make manual changes here!\npublic enum StatTypes\n{\n";
        for (int i = 0; i < statsList.Count; i++)
        {
            output += ("    " + statsList[i] + ",\n"); 
        }
        output += "}";

        // this check is only done to avoid executing twice, first time on script save and second time on unity editor click/focus - this seems as a bug.
        if (statsList.Count > 0)
        {
            Debug.Log(output);
            File.WriteAllText("Assets/Scripts/Stats/StatTypes.cs", output);
        }
    }
}
