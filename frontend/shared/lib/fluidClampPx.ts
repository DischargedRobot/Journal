type FluidClampPxOptions = {
	minViewport?: number
	maxViewport?: number
}

/** clamp(min, fluid, max): min на minViewport, max на maxViewport */
export const fluidClamp = (
	minSize: number,
	maxSize: number,
	{ minViewport = 320, maxViewport = 1920 }: FluidClampPxOptions = {},
): string => {
	const viewportSpan = maxViewport - minViewport
	const sizeSpan = maxSize - minSize

	return `clamp(${minSize}px, calc(${minSize}px + ${sizeSpan} * (100vw - ${minViewport}px) / ${viewportSpan}), ${maxSize}px)`
}
