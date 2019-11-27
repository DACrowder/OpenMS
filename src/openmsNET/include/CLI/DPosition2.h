#pragma once
#include <msclr/marshal_cppstd.h>

#include <OpenMS/DATASTRUCTURES/DPosition.h>
namespace OpenMSNET 
{
	public ref class DPosition2
	{
	public:
		OpenMS::DPosition<2>* inst;

		~DPosition2();
		DPosition2();
		DPosition2(OpenMS::DPosition<2>& dp);
		DPosition2(OpenMS::DPosition<2>&& dp);
		DPosition2(double x, double y);

		property double  X
		{
			void set(double x);
			double get();
		}

		property double  Y
		{
			void set(double y);
			double get();
		}

		bool operator==(DPosition2% point) { return this->inst == point.inst; }
		bool operator!=(DPosition2% point) { return this->inst != point.inst; }
		bool operator<(DPosition2% point) { return this->inst < point.inst; }
		bool operator<=(DPosition2% point) { return this->inst <= point.inst; }
		bool operator>(DPosition2% point) { return this->inst <= point.inst; }
		bool operator>=(DPosition2% point) { return this->inst <= point.inst; }

		bool SpatiallyLessEqual(DPosition2^ point);
		bool SpatiallyGreaterEqual(DPosition2^ point);

		DPosition2^ operator+(DPosition2^ point)
		{
			OpenMS::DPosition<2> dp = *(this->inst) + *(point->inst);
			return gcnew DPosition2(dp);
		}

		DPosition2% operator+=(DPosition2^ point)
		{
			*(this->inst) += *(point->inst);
			return *this;
		}

		DPosition2^ operator-(DPosition2% point)
		{
			OpenMS::DPosition<2> dp = *(this->inst) - *(point.inst);
			return gcnew DPosition2(dp);
		}

		DPosition2% operator-=(DPosition2^ point)
		{
			*(this->inst) -= *(point->inst);
			return *this;
		}

		DPosition2^ operator-()
		{
			return gcnew DPosition2(-(*(this->inst)));
		}

		double operator*(DPosition2^point)
		{
			return *(this->inst) * (*(point->inst));
		}

		DPosition2% operator*=(double scalar)
		{
			*(this->inst) *= scalar;
			return *this;
		}

		DPosition2% operator/=(double scalar)
		{
			*(this->inst) /= scalar;
			return *this;
		}

		static property DPosition2^ Zero
		{
			DPosition2^ get()
			{
				OpenMS::DPosition<2> mp = OpenMS::DPosition<2>::zero();
				DPosition2^ dp = gcnew DPosition2(mp);
				return dp;
			}
		}

		static property DPosition2^ MinPositive
		{
			DPosition2^ get()
			{
				OpenMS::DPosition<2> mp = OpenMS::DPosition<2>::minPositive();
				DPosition2^ dp = gcnew DPosition2(mp);
				return dp;
			}
		}

		static property DPosition2^ MinNegative
		{
			DPosition2^ get()
			{
				OpenMS::DPosition<2> mp = OpenMS::DPosition<2>::minNegative();
				DPosition2^ dp = gcnew DPosition2(mp);
				return dp;
			}
		}

		static property DPosition2^ MaxPositive
		{
			DPosition2^ get()
			{
				OpenMS::DPosition<2> mp = OpenMS::DPosition<2>::maxPositive();
				DPosition2^ dp = gcnew DPosition2(mp);
				return dp;
			}
		}
	protected:
		!DPosition2();
	private:
		bool mIsDisposed;
	};
}