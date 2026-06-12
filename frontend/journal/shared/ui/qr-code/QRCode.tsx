"use client"

import { useEffect, useRef } from "react"
import QRCodeStyling from "qr-code-styling"

interface Props {
	value: string
}

const QR_PLACEHOLDER = " "

const QRCode = ({ value }: Props) => {
	const containerRef = useRef<HTMLDivElement>(null)
	const qrCodeRef = useRef<QRCodeStyling | null>(null)
	const isEmpty = !value

	useEffect(() => {
		const container = containerRef.current
		if (!container) {
			return
		}

		const qrCode = new QRCodeStyling({
			width: 100,
			height: 100,
			data: value || QR_PLACEHOLDER,
			margin: 5,
			type: "svg",
			shape: "square",
			dotsOptions: {
				color: isEmpty
					? "var(--mui-palette-action-disabled)"
					: "var(--mui-palette-primary-contrastText)",
			},
			cornersDotOptions: {
				type: "dot",
				color: isEmpty
					? "var(--mui-palette-action-disabled)"
					: "var(--mui-palette-primary-contrastText)",
			},
			cornersSquareOptions: {
				type: "extra-rounded",
				color: isEmpty
					? "var(--mui-palette-action-disabled)"
					: "var(--mui-palette-primary-main)",
			},
		})

		qrCodeRef.current = qrCode
		container.innerHTML = ""
		qrCode.append(container)

		// обновляем размер QR кода при изменении размера контейнера
		const updateSize = () => {
			const size = container.offsetWidth
			if (size > 0) {
				qrCode.update({ width: size, height: size })
			}
		}

		updateSize()

		const observer = new ResizeObserver(updateSize)
		observer.observe(container)

		return () => {
			observer.disconnect()
			qrCodeRef.current = null
		}
	}, [value, isEmpty])

	return (
		<div
			ref={containerRef}
			style={{ height: "100%", aspectRatio: "1 / 1" }}
		/>
	)
}

export default QRCode
