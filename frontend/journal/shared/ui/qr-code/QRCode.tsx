"use client"

import { useEffect, useRef } from "react"
import QRCodeStyling from "qr-code-styling"

interface Props {
    value: string
}

const QRCode = ({ value }: Props) => {
    const ref = useRef<HTMLDivElement>(null)

    useEffect(() => {
        if (ref.current) {
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
                }
            })
            ref.current.innerHTML = ""
            qrCode.append(ref.current)
        }
    }, [value])

    return <div ref={ref} />
}

export default QRCode