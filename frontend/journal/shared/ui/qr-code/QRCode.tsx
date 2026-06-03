"use client"

import { useEffect, useRef } from "react"
import QRCodeStyling from "qr-code-styling"

interface Props {
    value: string
}

const QRCode = ({ value }: Props) => {
    const containerRef = useRef<HTMLDivElement>(null)
    const qrCodeRef = useRef<QRCodeStyling | null>(null)

    useEffect(() => {
        const container = containerRef.current
        if (!container) {
            return
        }

        const qrCode = new QRCodeStyling({
            width: 100,
            height: 100,
            data: value,
            margin: 5,
            type: "svg",
            shape: "square",
            cornersDotOptions: {
                type: "dot",
            },
            cornersSquareOptions: {
                type: "extra-rounded",
                color: "var(--mui-palette-primary-main)",
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
    }, [value])

    return (
        <div
            ref={containerRef}
            style={{ height: "100%", aspectRatio: "1 / 1" }}
        />
    )
}

export default QRCode