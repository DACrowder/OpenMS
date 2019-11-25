#pragma once
#include <OpenMSNET/CLI/ScopedPointer.h>
#include <OpenMSNET/CLI/DPosition.h>
#include <OpenMS/KERNEL/Peak1D.h>
#include <msclr/lock.h>
namespace OpenMSNET
{
  public ref class Peak1D
  {

  private:
	  bool mIsDisposed;
	  ScopedPointer<OpenMS::Peak1D*>^ inst;

  public:
    inline Peak1D()
    {
      this->mIsDisposed = false;
      this->inst = gcnew ScopedPointer<OpenMS::Peak1D*>(new OpenMS::Peak1D);
    }

    inline Peak1D(double mz, float intensity)
    {
      this->mIsDisposed = false;
      this->inst = gcnew ScopedPointer<OpenMS::Peak1D*>(new OpenMS::Peak1D(mz, intensity));
    }

    inline Peak1D(OpenMS::Peak1D& p)
    {
      this->mIsDisposed = false;
      this->inst = gcnew ScopedPointer<OpenMS::Peak1D*>(new OpenMS::Peak1D(p));
    }

    ~Peak1D();

    property float Intensity;

    property double MZ;

    double Pos;

    property DPosition<1, double> Position;

    Peak1D% operator=(const Peak1D% rhs);

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

  };
}

