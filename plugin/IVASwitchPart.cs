using System;
using UnityEngine;

namespace Reviva
{
    /*
     * Fairly simple part module which can switch an INTERNAL module on loading a vessel.
     */
    public class IVASwitchPart : PartModule
    {
        [KSPField(isPersistant = true)]
        public string internalName = null;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            IVASwitch();
        }

        /*  -------------------------------------------------------------------------------- */

	private void IVASwitch()
        {
            string oldName = GetCurrentInternalName();
            string newName = GetRequiredInternalName();

            Log($"Switch IVA {oldName} -> {newName}");
            if (newName == "")
            {
                LogError("internalName is null or empty, no switch");
                return;
            }
            if (oldName == newName)
            {
                Log("internalName unchanged, no switch");
                return;
            }

            UnloadIVA();
            UpdateInternalConfig(oldName, newName);
            RefreshInternalModel();
            LoadIVA();
        }


        private string GetCurrentInternalName()
        {
            ConfigNode internalConfig = this.part?.partInfo?.internalConfig;
            return GetConfigValue(internalConfig, "name");
        }

        private string GetConfigValue(ConfigNode node, string id)
        {
            if (node == null || !node.HasValue(id))
                return "";
            return node.GetValue(id) ?? "";
        }

        private string GetRequiredInternalName()
        {
            return this.internalName ?? "";
        }

        private void UnloadIVA()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                Log("Not in flight scene, IVA not unloaded");
                return;
            }

            Log("Unload in-flight IVA");
            this.part.DespawnIVA();
        }

        private void UpdateInternalConfig(string oldName, string newName)
        {
            if (this.part?.partInfo == null)
            {
                LogError("No part or partinfo, cannot switch");
                return;
            }

            ConfigNode newInternalConfig = new ConfigNode("INTERNAL");
            newInternalConfig.AddValue("name", newName);
            this.part.partInfo.internalConfig = newInternalConfig;
        }

        private void RefreshInternalModel()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                Log("Not in flight scene, internal model not refreshed");
                return;
            }
            if (this.part == null)
            {
                LogError("No current part, internal model not refreshed");
                return;
            }

            Log("Refresh IVA interal model");
            this.part.CreateInternalModel();
        }

        private void LoadIVA()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                Log("Not in flight scene, IVA not loaded");
                return;
            }

            Log("Load in-flight IVA");
            this.part.SpawnIVA();
        }

        private void Log(string text)
        {
            Debug.Log($"[Reviva] {text}");
        }

        private void LogError(string text)
        {
            Debug.LogError($"[Reviva] {text}");
        }
    }
}
