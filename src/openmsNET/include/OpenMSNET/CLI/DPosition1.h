#pragma once
#include <msclr/marshal_cppstd.h>

#include <OpenMS/DATASTRUCTURES/DPosition.h>

namespace OpenMSNET
{
	public ref class DPosition1
	{
	public:
		OpenMS::DPosition<1>* inst;

		~DPosition1();

		DPosition1();
		DPosition1(OpenMS::DPosition<1>& dp);
		DPosition1(OpenMS::DPosition<1>&& dp);
		DPosition1(double x);
		
		property double  X
		{
			void set(double x) { this->inst->setX(x); }
			double get() { return this->inst->getX(); }
		}

		inline bool operator==(DPosition1% point) { return this->inst == point.inst; }
		inline bool operator!=(DPosition1% point) { return this->inst != point.inst; }
		inline bool operator<(DPosition1% point) { return this->inst < point.inst; }
		inline bool operator<=(DPosition1% point) { return this->inst <= point.inst; }
		inline bool operator>(DPosition1% point) { return this->inst <= point.inst; }
		inline bool operator>=(DPosition1% point) { return this->inst <= point.inst; }

		bool SpatiallyLessEqual(DPosition1^ point);
		bool SpatiallyGreaterEqual(DPosition1^ point);

		inline DPosition1^ operator+(DPosition1^ point)
		{
			OpenMS::DPosition<1> dp = *(this->inst) + *(point->inst);
			return gcnew DPosition1(dp);
		}

		inline DPosition1% operator+=(DPosition1^ point)
		{
			*(this->inst) += *(point->inst);
			return *this;
		}

		inline DPosition1^ operator-(DPosition1% point)
		{
			OpenMS::DPosition<1> dp = *(this->inst) - *(point.inst);
			return gcnew DPosition1(dp);
		}

		inline DPosition1% operator-=(DPosition1^ point)
		{
			*(this->inst) -= *(point->inst);
			return *this;
		}

		inline DPosition1^ operator-()
		{
			return gcnew DPosition1(-(*(this->inst)));
		}

		inline double operator*(DPosition1^point)
		{
			return *(this->inst) * (*(point->inst));
		}

		inline DPosition1% operator*=(double scalar)
		{
			*(this->inst) *= scalar;
			return *this;
		}

		inline DPosition1% operator/=(double scalar)
		{
			*(this->inst) /= scalar;
			return *this;
		}

		static property DPosition1^ Zero
		{
			DPosition1^ get()
			{
				OpenMS::DPosition<1> mp = OpenMS::DPosition<1>::zero();
				DPosition1^ dp = gcnew DPosition1(mp);
				return dp;
			}
		}

		static property DPosition1^ MinPositive
		{
			DPosition1^ get()
			{
				OpenMS::DPosition<1> mp = OpenMS::DPosition<1>::minPositive();
				DPosition1^ dp = gcnew DPosition1(mp);
				return dp;
			}
		}

		static property DPosition1^ MinNegative
		{
			DPosition1^ get()
			{
				OpenMS::DPosition<1> mp = OpenMS::DPosition<1>::minNegative();
				DPosition1^ dp = gcnew DPosition1(mp);
				return dp;
			}
		}

		static property DPosition1^ MaxPositive
		{
			DPosition1^ get()
			{
				OpenMS::DPosition<1> mp = OpenMS::DPosition<1>::maxPositive();
				DPosition1^ dp = gcnew DPosition1(mp);
				return dp;
			}
		}
	protected:
		!DPosition1();
	private:
		bool mIsDisposed;
	};
}