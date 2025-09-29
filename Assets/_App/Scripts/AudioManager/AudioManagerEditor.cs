#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(20); // Add 20 pixels of space before the button
        EditorGUILayout.LabelField("EDITOR", EditorStyles.boldLabel);
        AudioManager audioManager = (AudioManager)target;
        EditorGUILayout.HelpBox("Drag audio clips to Music Clip List and Sound Clip List then press reload to generate sound enum!",
            MessageType.Info);
        if (GUILayout.Button("Reload"))
        {
            GenerateMusicEnum(audioManager.MusicClips);
            GenerateSoundEnum(audioManager.SoundClips);
        }
    }

    private void GenerateMusicEnum(List<AudioClip> soundClips) => GenerateOrUpdateEnumFile("EMusic", soundClips.ToArray());
    private void GenerateSoundEnum(List<AudioClip> soundClips) => GenerateOrUpdateEnumFile("ESound", soundClips.ToArray());

    private string GetEnumContent(string enumName, AudioClip[] soundClips)
    {
        var enumContent = $"public enum {enumName}\n{{\n";
        enumContent += "    None,\n";

        // HashSet to track duplicate names
        var clipNames = new HashSet<string>();

        foreach (AudioClip clip in soundClips)
        {
            if (!clipNames.Add(clip.name))
            {
                Debug.LogError($"Duplicate enum entry found: {clip.name}");
            }
            else
            {
                enumContent += $"    {RemoveDiacritics(clip.name)},\n";
            }
        }

        enumContent += "}\n";

        return enumContent;
    }

    private string RemoveDiacritics(string s)
    {
        return string.Concat(s.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            .Normalize(NormalizationForm.FormC).Replace(" ", "_");
    }

    private void GenerateOrUpdateEnumFile(string enumName, AudioClip[] soundClips)
    {
        try
        {
            // Search for existing enum file
            string[] files = Directory.GetFiles(Application.dataPath, $"{enumName}.cs", SearchOption.AllDirectories);

            // Generate the enum content
            string enumContent = GetEnumContent(enumName, soundClips);

            if (files.Length > 0)
            {
                // If there are existing ESound.cs files, overwrite the first found file
                string filePath = files[0];
                File.WriteAllText(filePath, enumContent);
                Debug.Log($"{enumName}.cs updated successfully at: {filePath}");
            }
            else
            {
                // If no existing ESound.cs file found, create a new one
                string filePath = Path.Combine(Application.dataPath, "Scripts", $"{enumName}.cs");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, enumContent);
                Debug.Log($"{enumName}.cs created successfully at: {filePath}");
            }

            // Refresh AssetDatabase to apply changes
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to generate/update {enumName}.cs: {e.Message}");
        }
    }
}
#endif