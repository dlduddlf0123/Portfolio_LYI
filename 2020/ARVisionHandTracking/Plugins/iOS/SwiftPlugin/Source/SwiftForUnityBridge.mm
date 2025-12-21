//
//  SwiftForUnityBridge.m
//  iOS-UnityText
//
//  Created by one bob on 2020/06/25.
//  Copyright © 2020 one bob. All rights reserved.
//

#include "SwiftPlugin-Swift.h"

#import <Foundation/Foundation.h>
#import "UnityXRNativePtrs.h"
#import <ARKit/ARKit.h>
#import <Metal/Metal.h>
#import <AVFoundation/AVFoundation.h>
#import "UnityAppController.h"
#include "Unity/IUnityInterface.h"


extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces);
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload();

@interface MyAppController : UnityAppController
{
}
- (void)shouldAttachRenderDelegate;
@end
@implementation MyAppController
- (void)shouldAttachRenderDelegate;
{
    UnityRegisterRenderingPluginV5(&UnityPluginLoad, &UnityPluginUnload);
}
@end
IMPL_APP_CONTROLLER_SUBCLASS(MyAppController);


//region fields
static id<MTLTexture> s_CaptureTexture = nil;
static CVMetalTextureCacheRef s_TextureCache = nil;
static id <MTLDevice> s_Device = nil;
static CVPixelBufferRef s_PixelBuffer = nil;

static IUnityInterfaces*    s_UnityInterfaces   = 0;

#pragma mark - C interface

extern "C" {

bool SwiftUnity_StartDetect(intptr_t ptr) {
    
    // In case of invalid buffer ref
    if (!ptr) return 0;
    
    UnityXRNativeFrame_1* unityXRFrame = (UnityXRNativeFrame_1*) ptr;
    ARFrame* frame = (__bridge ARFrame*)unityXRFrame->framePtr;
    
    CVPixelBufferRef buffer = frame.capturedImage;
    // Forward message to the swift api
    return [[SwiftForUnity shared] startDetectionWithBuffer: buffer] ? 1 : 0;
}



}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
    s_Device = MTLCreateSystemDefaultDevice();
    CVMetalTextureCacheCreate(NULL, NULL, s_Device, NULL, &s_TextureCache);
    AVCaptureVideoDataOutput *outputCapture = [[AVCaptureVideoDataOutput alloc] init];
    outputCapture.videoSettings = @{
        (NSString *)kCVPixelBufferMetalCompatibilityKey: @YES,
        (NSString*)kCVPixelBufferPixelFormatTypeKey : @(kCVPixelFormatType_420YpCbCr8BiPlanarVideoRange)
    };
    
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
}


