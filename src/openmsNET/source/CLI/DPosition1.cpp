#pragma once
#include <CLI/DPosition1.h>
namespace OpenMSNET
{
	DPosition1::DPosition1()
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<1>;
	}

	DPosition1::DPosition1(OpenMS::DPosition<1>& dp)
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<1>(dp);
	}

	DPosition1::DPosition1(OpenMS::DPosition<1>&& dp)
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<1>(dp);
	}
	
	DPosition1::DPosition1(double x)
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<1>(x);
	}

	DPosition1::~DPosition1() 
	{ 
		this->!DPosition1();
	}

	DPosition1::!DPosition1()
	{
		if (!this->mIsDisposed)
		{
			delete this->inst;
			this->mIsDisposed = true;
		}
	}

	bool DPosition1::SpatiallyLessEqual(DPosition1^ point)
	{
		bool yes = this->inst->spatiallyLessEqual(*(point->inst));
		return yes;
	}

	bool DPosition1::SpatiallyGreaterEqual(DPosition1^ point)
	{
		bool yes = this->inst->spatiallyGreaterEqual(*(point->inst));
		return yes;
	}

	DPosition1^ DPosition1::Zero::get()
	{
		OpenMS::DPosition<1>* mp = new OpenMS::DPosition<1>(OpenMS::DPosition<1>::zero());
		DPosition1^ dp = gcnew DPosition1(*mp);
		return dp;
	}

	DPosition1^ DPosition1::MinPositive::get()
	{
		OpenMS::DPosition<1>* mp = new OpenMS::DPosition<1>(OpenMS::DPosition<1>::minPositive());
		DPosition1^ dp = gcnew DPosition1(*mp);
		return dp;
	}

	DPosition1^ DPosition1::MinNegative::get()
	{
		OpenMS::DPosition<1>* mp = new OpenMS::DPosition<1>(OpenMS::DPosition<1>::minNegative());
		DPosition1^ dp = gcnew DPosition1(*mp);
		return dp;
	}

	DPosition1^ DPosition1::MaxPositive::get()
	{
		OpenMS::DPosition<1>* mp = new OpenMS::DPosition<1>(OpenMS::DPosition<1>::maxPositive());
		DPosition1^ dp = gcnew DPosition1(*mp);
		return dp;
	}
	

}