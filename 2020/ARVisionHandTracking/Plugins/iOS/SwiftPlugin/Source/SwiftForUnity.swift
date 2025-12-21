//
//  SwiftForUnity.swift
//  iOS-UnityText
//
//  Created by one bob on 2020/06/25.
//  Copyright © 2020 one bob. All rights reserved.
//


import UIKit
import Vision
import AVFoundation
import CoreImage
import Foundation


@objc public class SwiftForUnity: UIViewController{
    
    @objc static let shared = SwiftForUnity()
    
    // Unique serial queue reserved for vision requests
    private let visionQueue = DispatchQueue(label: "com.AroundEffect.UnityVision")
    
    // Used to reatain the pixel buffer when it is passed in as a target to evaulate
    private var retainedBuffer: CVPixelBuffer?
    @objc public var outputBuffer : CVPixelBuffer?
    
    // Id of the managed Unity game object to forward messages to
    private var callbackTarget: String = "SwiftUnity"
    
    private lazy var predictionRequest: VNCoreMLRequest = {
        // Load the ML model through its generated class and create a Vision request for it.
        do {
            let model = try VNCoreMLModel(for:  HandModel().model)
            let request = VNCoreMLRequest(model: model, completionHandler: handRecognitionCompleteHandler)
            
            request.imageCropAndScaleOption = VNImageCropAndScaleOption.scaleFill
            return request
        } catch {
            fatalError("can't load Vision ML model: \(error)")
        }
    }()
    
    
    //region methods
    @objc func startDetection(buffer: CVPixelBuffer) -> Bool {
        //TODO
        self.retainedBuffer = buffer
        let imageRequestHandler = VNImageRequestHandler(cvPixelBuffer: self.retainedBuffer!, orientation: .right)
        
        visionQueue.async {
            do {
                defer { self.retainedBuffer = nil }
                try imageRequestHandler.perform([self.predictionRequest])
            } catch {
                fatalError("Perform Failed:\"\(error)\"")
            }
        }
        
        return true
    }
    
    private func handRecognitionCompleteHandler(request: VNRequest, error: Error?)
    {
        DispatchQueue.main.async {
        
        if(error != nil) {
            UnitySendMessage(self.callbackTarget, "OnHandDetecedFromNative", "")
            fatalError("error\(error)")
        }
        
        guard let observation = self.predictionRequest.results?.first as? VNPixelBufferObservation else {
            UnitySendMessage(self.callbackTarget, "OnHandDetecedFromNative", "")
            fatalError("Unexpected result type from VNCoreMLRequest")
        }

        let outBuffer = observation.pixelBuffer

        self.outputBuffer = observation.pixelBuffer

        guard let point = outBuffer.searchTopPoint() else{
            UnitySendMessage(self.callbackTarget, "OnHandDetecedFromNative", "")
            return
        }
            
        UnitySendMessage(self.callbackTarget, "OnHandDetecedFromNative", "\(point.y),\(point.x)")
            
//            guard let posObservations = self.predictionRequest.results?.first as? VNRecognizedObjectObservation else {
//                UnitySendMessage(self.callbackTarget, "OnHandDetecedFromNative", "")
//                return
//            }
//            let posBuffer = posObservations.boundingBox;
//            UnitySendMessage(self.callbackTarget, "OnHandDetecedFromNative", "\(posBuffer.midX),\(posBuffer.midY)")
            
    }
//        DispatchQueue.main.async {
//            if error != nil {
//                    let error = "[VisionNative] Error: " + (error?.localizedDescription)!
//                    UnitySendMessage(self.callbackTarget, "OnHandRecognitionComplete", error)
//                    return
//                }
//
//
//            guard let posObservations = request.results?.first as? VNRecognizedObjectObservation else {
//                UnitySendMessage(self.callbackTarget, "OnHandRecognitionComplete", "")
//                return
//            }
//            let posBuffer = posObservations.boundingBox;
//            let guestureBuffer = posObservations.labels.first?.identifier
//
//            UnitySendMessage(self.callbackTarget, "OnHandRecognitionComplete", "\(posBuffer.midX),\(posBuffer.midY),\(guestureBuffer ?? "1Hand")")
//        }
    }
}

