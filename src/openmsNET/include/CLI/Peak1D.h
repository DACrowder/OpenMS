#pragma once
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include <CLI/DPosition1.h>

#include <OpenMS/KERNEL/Peak1D.h>
namespace OpenMSNET
{
  public ref class Peak1D
  {
  public:
	Peak1D();
	Peak1D(double mz, float intensity);
	Peak1D(OpenMS::Peak1D& p);
	
	~Peak1D();

	OpenMS::Peak1D* inst;

	property float Intensity
	{
		void set(float i);
		float get();
	};

	property double MZ
	{
		void set(double mz);
		double get();
	};

	property double Pos
	{
		void set(double p);
		double get();
	};

	property DPosition1^ Position
	{
		void set(DPosition1^ p);
		DPosition1^ get();
	};

    inline bool MZLess(Peak1D% lhs, Peak1D% rhs) { return lhs.MZ < rhs.MZ; }
    inline bool MZLess(Peak1D% lhs, double rhs) { return lhs.MZ < rhs; }
    inline bool MZLess(double lhs, Peak1D% rhs) { return lhs < rhs.MZ; }
    inline bool MZLess(double lhs, double rhs) { return lhs < rhs; }

    inline bool PositionLess(Peak1D% lhs, Peak1D% rhs) { return lhs.Pos < rhs.Pos; }
    inline bool PositionLess(Peak1D% lhs, double rhs) { return lhs.Pos < rhs; }
    inline bool PositionLess(double lhs, Peak1D% rhs) { return lhs < rhs.Pos; }
    inline bool PositionLess(double lhs, double rhs) { return lhs < rhs; }

    inline bool IntensityLess(Peak1D% lhs, Peak1D% rhs) { return lhs.Intensity < rhs.Intensity; }
    inline bool IntensityLess(Peak1D% lhs, float rhs) { return lhs.Intensity < rhs; }
    inline bool IntensityLess(float lhs, Peak1D% rhs) { return lhs < rhs.Intensity; }
    inline bool IntensityLess(float lhs, float rhs) { return lhs < rhs; }

  protected:
	  !Peak1D();
  private:
	  bool mIsDisposed;
  };
}

