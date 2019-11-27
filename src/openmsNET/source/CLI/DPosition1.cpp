#pragma once
#include <OpenMSNET/CLI/DPosition1.h>
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
}