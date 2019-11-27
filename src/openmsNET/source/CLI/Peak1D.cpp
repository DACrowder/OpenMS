#include <OpenMS/CLI/DPosition1.h>
#include <OpenMS/CLI/Peak1D.h>

OpenMSNET::Peak1D::Peak1D()
{
	this->mIsDisposed = false;
	this->inst = new OpenMS::Peak1D;
}

OpenMSNET::Peak1D::Peak1D(double mz, float intensity)
{
	this->mIsDisposed = false;
	this->inst = new OpenMS::Peak1D(mz, intensity);
}

OpenMSNET::Peak1D::Peak1D(OpenMS::Peak1D& p)
{
	this->mIsDisposed = false;
	this->inst = new OpenMS::Peak1D(p);
}

OpenMSNET::Peak1D::~Peak1D() { this->!Peak1D(); }

OpenMSNET::Peak1D::!Peak1D()
{
	if (!this->mIsDisposed)
	{
		delete this->inst;
		this->mIsDisposed = true;
	}
}

float OpenMSNET::Peak1D::Intensity::get() 
{
	OpenMS::Peak1D* p = this->inst;
	return p->getIntensity();
}
void OpenMSNET::Peak1D::Intensity::set(float i) 
{ 
	OpenMS::Peak1D* p = this->inst;
	p->setIntensity(i); 
}

double OpenMSNET::Peak1D::MZ::get() 
{ 
	OpenMS::Peak1D* p = this->inst;
	return p->getMZ();
}
void OpenMSNET::Peak1D::MZ::set(double mz) 
{
	OpenMS::Peak1D* p = this->inst;
	p->setMZ(mz);
}

double OpenMSNET::Peak1D::Pos::get() 
{ 
	OpenMS::Peak1D* p = this->inst;
	return p->getPos();
}
void OpenMSNET::Peak1D::Pos::set(double p)
{
	OpenMS::Peak1D* ptr = this->inst;
	ptr->setPos(p);
}

void OpenMSNET::Peak1D::Position::set(OpenMSNET::DPosition1^ dp) 
{ 
	OpenMS::Peak1D *p = this->inst;
	OpenMS::DPosition<1> *dptr = dp->inst;
	p->setPosition(*dptr);
}

OpenMSNET::DPosition1^ OpenMSNET::Peak1D::Position::get() 
{ 
	OpenMS::Peak1D* p = this->inst;
	return gcnew DPosition1(p->getPosition());
}
