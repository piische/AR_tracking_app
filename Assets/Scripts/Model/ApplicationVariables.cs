using System.Runtime.Serialization;

namespace ApplicationVariables { 

    public enum ApplicationState {
        QR_READING,
        LOADING,
        NONE,
        DESCRIPTION,
        MATERIALCHECK,
        PRODUCTION,
        GAME,
        IMPLANTVIEW,
        ERROR
    }

    public enum ArContentType {
        [EnumMember(Value = "PROCEDURAL")]
        PROCEDURAL,
        [EnumMember(Value = "MOLECULAR")]
        MOLECULAR,
        [EnumMember(Value = "MEDICAL")]
        MEDICAL
    }

    public enum ArObjectType {
        SCENE,
        STEP
    }

    public enum ApplicationMode {
        NONE,
        SOP,
        MOLECULE,
        MEDICAL
    }

    public enum WarningType {
        [EnumMember(Value = "NONE")]
        NONE = 0,
        [EnumMember(Value = "R_PHRASES")]
        R_PHRASES = 1,
        [EnumMember(Value = "S_PHRASES")]
        S_PHRASES = 2,
        [EnumMember(Value = "WARNING")]
        WARNING = 3
    }

    public enum AtomType {
        C,
        N,
        O,
        H,
        S,
        P,
        Default
    }
    public enum BondType {
        SIMPLE = 1,
        DOUBLE = 2,
        TRIPPLE = 3,
        AROMATIC = 4
    }

    public struct SimpleAtom {
        public float X;
        public float Y;
        public float Z;
        public AtomType AtomType;
    }

    public struct SimpleBond {
        public int atom1;
        public int atom2;
        public BondType bond;
    }
}
