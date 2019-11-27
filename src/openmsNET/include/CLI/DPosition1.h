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

		bool operator==(DPosition1% point) { return this->inst == point.inst; }
		bool operator!=(DPosition1% point) { return this->inst != point.inst; }
		bool operator<(DPosition1% point) { return this->inst < point.inst; }
		bool operator<=(DPosition1% point) { return this->inst <= point.inst; }
		bool operator>(DPosition1% point) { return this->inst <= point.inst; }
		bool operator>=(DPosition1% point) { return this->inst <= point.inst; }

		bool SpatiallyLessEqual(DPosition1^ point);
		bool SpatiallyGreaterEqual(DPosition1^ point);

		DPosition1^ operator+(DPosition1^ point)
		{
			OpenMS::DPosition<1> dp = *(this->inst) + *(point->inst);
			return gcnew DPosition1(dp);
		}

		DPosition1% operator+=(DPosition1^ point)
		{
			*(this->inst) += *(point->inst);
			return *this;
		}

		DPosition1^ operator-(DPosition1% point)
		{
			OpenMS::DPosition<1> dp = *(this->inst) - *(point.inst);
			return gcnew DPosition1(dp);
		}

		DPosition1% operator-=(DPosition1^ point)
		{
			*(this->inst) -= *(point->inst);
			return *this;
		}

		DPosition1^ operator-()
		{
			return gcnew DPosition1(-(*(this->inst)));
		}

		double operator*(DPosition1^point)
		{
			return *(this->inst) * (*(point->inst));
		}

		DPosition1% operator*=(double scalar)
		{
			*(this->inst) *= scalar;
			return *this;
		}

		DPosition1% operator/=(double scalar)
		{
			*(this->inst) /= scalar;
			return *this;
		}

		static property DPosition1^ Zero
		{
			DPosition1^ get();
		}

		static property DPosition1^ MinPositive
		{
			DPosition1^ get();
		}

		static property DPosition1^ MinNegative
		{
			DPosition1^ get();
		}

		static property DPosition1^ MaxPositive
		{
			DPosition1^ get();
		}
	protected:
		!DPosition1();
	private:
		bool mIsDisposed;
	};
}