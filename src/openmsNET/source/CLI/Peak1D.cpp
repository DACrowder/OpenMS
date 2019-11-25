#pragma once
#include <OpenMSNET/CLI/Peak1D.h>

namespace OpenMSNET
{
	public ref class OpenMSNET::Peak1D
  {
    public:
		~Peak1D() 
		{
			this->!Peak1D();
		}

      property float Intensity
      {
		  void set(float i) { inst->get->setIntensity(i); }
        float get() { return this->inst->get->getIntensity(); }
      };

      property double MZ
      {
        void set(double mz) { this->inst->get->setMZ(mz); }
        double get() { return this->inst->get->getMZ(); }
      };

      property double Pos
      {
        void set(double p) { this->inst->get->setPos(p); }
        double get() { return this->inst->get->getPos(); }
      };

      property DPosition<1, double>^ Position
      {
        void set(DPosition<1, double>^ p) { this->inst->get->setPosition(p); }
		    DPosition<1, double>^ get() { return this->inst->get->getPosition(); }
      };

    protected:
      !Peak1D()
      {
		  if (!mIsDisposed)
		  {
			  delete this->inst;
			  this->mIsDisposed = true;
		  }
      }
	  private:
		  bool mIsDisposed;
		  ScopedPointer<OpenMS::Peak1D*> ^inst;
  };
};