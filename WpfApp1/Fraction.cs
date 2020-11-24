using System;

public class atom_Fraction
{
	double atom_Numerator;
	double atom_Denomintaor;
	public atom_Fraction(double atom_num, double atom_denom)
	{
		atom_Numerator = atom_num;
		atom_Denominator = atom_denom
	}

	public atom_Fraction add(atom_Fraction fraction)
	{
		double atom_Num   = atom_Numerator * fraction.atom_Denominator + atom_Denomintaor * fraction.atom_Numerator;
		double atom_Denom = atom_Denomintaor * fraction.atom_Denomintaor;
		atom_Fraction frac = new atom_Fraction(atom_Num, atom_Denom);
		return frac;
	}


}
