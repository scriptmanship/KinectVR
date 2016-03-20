#import <UIKit/UIKit.h>
#import "UnityAppController.h"

extern "C" void MMTUnitySetGraphicsDevice(void* device, int deviceType, int eventType);
extern "C" void MMTUnityRenderEvent(int marker);

@interface MMTAppController : UnityAppController
{
}
- (void)shouldAttachRenderDelegate;
@end

@implementation MMTAppController

- (void)shouldAttachRenderDelegate;
{
	UnityRegisterRenderingPlugin(&MMTUnitySetGraphicsDevice, &MMTUnityRenderEvent);
}
@end


IMPL_APP_CONTROLLER_SUBCLASS(MMTAppController)

