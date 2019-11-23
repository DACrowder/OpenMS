#pragma once
#include <ScopedPointer.h>
#include <OpenMS/KERNEL/Peak1D.h>

namespace OpenMSNET
{
  public ref class Peak1D
  {
  public:
    inline Peak1D()
    {
      this->mIsDisposed = false;
      this->inst = new ScopedPointer(new OpenMS::Peak1D);
    }

    inline Peak1D(double mz, float intensity)
    {
      this->mIsDisposed = false;
      this->inst = new ScopedPointer(new OpenMS::Peak1D(mz, intensity));
    }

    inline Peak1D(OpenMS::Peak1D& p)
    {
      this->mIsDisposed = false;
      this->inst = new ScopedPointer(new OpenMS::Peak1D(p));
    }

    ~Peak1D()
    {
      !Peak1D();
    }

    inline property float Intensity
    {
      void set(float i) { this->inst->get()->setIntensity(i); }
      double get() { return this->inst->get()->getIntensity(); }
    };

    inline property double MZ
    {
      void set(double mz) { this->inst->get()->setMZ(mz); }
      double get() { return this->inst->get()->getMZ(); }
    };

    inline property double Pos
    {
      void set(double p) { this->inst->get()->setPos(p); }
      double get() { return this->inst->get()->getPos(); }
    };

    inline property DPosition<1, double>%^ Position
    {
      void set(DPosition<1, double>%^ p) { _position = p; }
      DPosition<1, double>%^ get() { return _position; }
    };

    inline Peak1D% operator=(const Peak1D% rhs)
    {
      if (this->inst->get() != rhs.inst->get())
      {
        this->inst->get() = rhs.inst->get();
      }
      return %this;
    }

    inline bool MZLess(Peak1D% lhs, Peak1D% rhs) { return lhs.MZ < rhs.MZ; }
    inline bool MZLess(Peak1D% lhs, double rhs) { return lhs.MZ < rhs; }
    inline bool MZLess(double lhs, Peak1D% rhs) { return lhs < rhs.MZ; }
    inline bool MZLess(double lhs, double rhs) { return lhs < rhs; }

    inline bool PositionLess(Peak1D% lhs, Peak1D% rhs) { return lhs.Position < rhs.Position; }
    inline bool PositionLess(Peak1D% lhs, double rhs) { return lhs.Position < rhs; }
    inline bool PositionLess(double lhs, Peak1D% rhs) { return lhs < rhs.Position; }
    inline bool PositionLess(double lhs, double rhs) { return lhs < rhs; }

    inline bool IntensityLess(Peak1D% lhs, Peak1D% rhs) { return lhs.Intensity < rhs.Intensity; }
    inline bool IntensityLess(Peak1D% lhs, float rhs) { return lhs.Intensity < rhs; }
    inline bool IntensityLess(float lhs, Peak1D% rhs) { return lhs < rhs.Intensity; }
    inline bool IntensityLess(float lhs, float rhs) { return lhs < rhs; }

  protected:
    !Peak1D()
    {
      lock mtx(mIsDisposed);
      if (!mIsDisposed)
      {
        delete this->inst;
        this->mIsDisposed = true;
      }
    }

  private:
    bool mIsDisposed;
    SharedPointer<OpenMS::Peak1D*>^ inst;

  };
}

