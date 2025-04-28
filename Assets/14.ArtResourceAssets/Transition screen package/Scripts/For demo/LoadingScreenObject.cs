using System;
using System.Collections.Generic;
using UnityEngine;

namespace TransitionScreenPackage.Data
{
    public enum TransitionScreenType
    {
        Type_1,
        Type_2,
        Type_3,
        Type_4,
        Type_5,
        Type_6,
        Type_7,
        Type_8,
        Type_9,
        Type_10, 
        Type_11,
        Type_12,
        Type_13,
        Type_14,
        Type_15,
        Type_16
    }
    
    public enum TransitionScreenVersion
    {
        Normal,
        Outlined
    }

    [Serializable]
    public struct TransitionScreenObject
    {
        public TransitionScreenType Type;
        public List<TransitionVersionObject> TransitionScreenVersions;

        public TransitionVersionObject GetVersion(TransitionScreenVersion version)
        {
            foreach (TransitionVersionObject versionObject in TransitionScreenVersions)
            {
                if (versionObject.Version == version)
                    return versionObject;
            }

            throw new Exception("Cannot find transition screen version " + version + " in type " + Type);
        }
    }
    
    [Serializable]
    public struct TransitionVersionObject
    {
        public TransitionScreenVersion Version;
        public GameObject PrefabObject;
    }
}



