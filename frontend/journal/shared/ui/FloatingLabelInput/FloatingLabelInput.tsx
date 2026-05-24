"use client"

import type { InputProps } from "@chakra-ui/react"
import {
    Box,
    Field,
    Input,
    defineStyle,
    useControllableState,
} from "@chakra-ui/react"
import { useState } from "react"


interface FloatingLabelInputProps extends InputProps {
    label: React.ReactNode
    value?: string | undefined
    defaultValue?: string | undefined
    onValueChange?: ((value: string) => void) | undefined
}

const FloatingLabelInput = (props: FloatingLabelInputProps) => {
    const { label, onValueChange, value, defaultValue = "", ...rest } = props

    const [inputState, setInputState] = useControllableState({
        defaultValue,
        onChange: onValueChange,
        value,
    })

    const [focused, setFocused] = useState(false)
    const shouldFloat = inputState.length > 0 || focused

    return (
        <Box pos="relative" w="full">
            <Input
                css={intputStyle}
                {...rest}
                onFocus={(e) => {
                    props.onFocus?.(e)
                    setFocused(true)
                }}
                onBlur={(e) => {
                    props.onBlur?.(e)
                    setFocused(false)
                }}
                onChange={(e) => {
                    props.onChange?.(e)
                    setInputState(e.target.value)
                }}
                value={inputState}
                data-float={shouldFloat || undefined}
            />
            <Field.Label css={floatingStyles} data-float={shouldFloat || undefined}>
                {label}
            </Field.Label>
        </Box>
    )
}

export default FloatingLabelInput

const intputStyle = defineStyle({
    background: "gray.50",
    borderColor: "gray.500",
})
const floatingStyles = defineStyle({
    pos: "absolute",
    bg: "bg",
    px: "0.5",
    top: "2.5",
    insetStart: "3",
    fontWeight: "normal",
    pointerEvents: "none",
    transition: "position",
    color: "gray.500",
    background: "opacity( 100%)",
    "&[data-float]": {
        top: "-3",
        insetStart: "2",
        color: "gray.700",
    },
})
