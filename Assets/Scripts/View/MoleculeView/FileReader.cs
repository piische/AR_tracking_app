using System;
using System.Collections.Generic;
using ApplicationVariables;
using System.Text.RegularExpressions;

/// <summary>
/// Script that handling the reading and parsing of the SDF file
/// <summary>
public class FileReader  {

    /// <summary>
    /// create a list of atoms from a String array
    /// string has to SDF formated atom information
    /// <summary>
    public static List<SimpleAtom> GetSimpleAtoms(String[] msg) {
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex("[ ]{2,}", options);
        List<SimpleAtom> atoms = new List<SimpleAtom>();

        foreach(string s in msg) {
            SimpleAtom atom = new SimpleAtom();
            AtomType atomType;

            string[] atomLine = regex.Replace(s.Trim(), " ").Split(' ');
            atom.X = float.Parse(atomLine[0], System.Globalization.CultureInfo.InvariantCulture);
            atom.Y = float.Parse(atomLine[1], System.Globalization.CultureInfo.InvariantCulture);
            atom.Z = float.Parse(atomLine[2], System.Globalization.CultureInfo.InvariantCulture);

            Enum.TryParse(atomLine[3], out atomType);
            atom.AtomType = atomType;
            atoms.Add(atom);
        }
        return atoms;
    }

    /// <summary>
    /// create a list of bonds from a string array
    /// string has to SDF formated bond information
    /// <summary>
    public static List<SimpleBond> GetSimpleBonds(String[] msg) {
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex("[ ]{2,}", options);
        List<SimpleBond> bonds = new List<SimpleBond>();
        foreach (string s in msg) {
            string[] bondLine = regex.Replace(s.Trim(), " ").Split(' ');
            SimpleBond bond = new SimpleBond();
            bond.atom1 = Int32.Parse(bondLine[0]) - 1;
            bond.atom1 = Int32.Parse(bondLine[1]) - 1;
            BondType bondType;
            Enum.TryParse(bondLine[2], out bondType);
            bond.bond = bondType;
            bonds.Add(bond);
        }
        return bonds;
    }
}
