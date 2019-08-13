Documentation Author: Alexander Amling

# Unity Compute Shaders

[part 2](https://github.com/IGME-RIT/unity-indirect-instanced-rendering)

*Unity Compute Shaders Part 3*

This demo builds off of where part 2 left off, and incorperates a compute shader that changes the transform of the object instances.

For the most part, the syntax of compute shaders in Unity is similar to the syntax of a typical surface shader since both are written in DX11 HLSL, but there are a few new concepts.

In Unity compute shaders, the functions of the shader are invoked by retrieving and executing the kernel of the desired function. A single compute shader can have up to 4 kernels, but for this demo we use just one. The shader requires a directive to compile each function. Ours looks likes this:
```
#pragma kernel Translate
...
[numthreads(64,1,1)]
void Translate (uint3 id : SV_DispatchThreadID)
{
	...
}
```
This secction of code has 2 other important details. The [numthreads()] statement, and uint3 id : SV_DispatchThreadID. Both of these relate to a rather complicated aspect of compute shaders. In short, numthreads allows you to specify the dimensions of the thread groups, and uint3 id: SV_DispatchThreadID is used to track the value of the current index (good for indexing into data). For our needs, we only need to iterate in a single dimension, and the x dimension is 64 because it is good practice to work around the minimum threadgroup size of AMD hardware; 64 (Nvidia is 32). It is a good rule of thumb that when working in a single dimension or with unrelated data points 64x1x1 is a good fit, when working with things like pixels in an image or points on a plane 8x8x1 is a good fit, and when working with values in 3D space 4x4x4 is a good fit (never use 1x1x1). Keep in mind, all of this completely depends on your specific application, and I highly recommend that you do some research on your own (see [microsoft documentation](https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/sm5-attributes-numthreads))

To use our compute shader, we have to fetch the desired kernel from the shader
```
translationKernel = transformationShader.FindKernel("Translate");
```
Provide it with the nessicary buffers (this is the *exact* same transform buffer that is provided to the material)
```
transformationShader.SetBuffer(translationKernel, "transformBuffer", transformBuffer);
...
transformationShader.SetBuffer(translationKernel, "translationBuffer", translationBuffer);
```
And dispatch the kernel, providing the number of thread groups we will be sending
```
transformationShader.Dispatch(translationKernel, instanceCount / 64, 1, 1);
```
Inside the compute shader, we are taking the same transform matricies that we used in part 2, and adding to the position values over time. Notice how we are only using id.x as our value to index into our data, since the threadgroups are in a 64x1x1 arrangement.
	
	
- [More information on indexing into matricies in HLSL](https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-per-component-math#the-matrix-type) 
- [More information on translation matricies](https://www.brainvoyager.com/bv/doc/UsersGuide/CoordsAndTransforms/SpatialTransformationMatrices.html)
```
void Translate (uint3 id : SV_DispatchThreadID)
{
	transformBuffer[id.x]._14 += translationBuffer[id.x].x * deltaTime;
	transformBuffer[id.x]._24 += translationBuffer[id.x].y * deltaTime;
	transformBuffer[id.x]._34 += translationBuffer[id.x].z * deltaTime;
}
```
You may have noticed that there is an inconsistancy in the code. In the C# portion, the translation buffer is fed an array of floats, and in the compute shader, the translation buffer is using float3. This is because data that is passed to a buffer is unformatted. If the shader is expecting a buffer of a 4x4 matrix, whether you send it a 4x4 matrix, or 16 floats, the data is read in the same. We will look into some of the uses of this in part 4.
