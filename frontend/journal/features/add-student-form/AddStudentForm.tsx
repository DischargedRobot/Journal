import { Box } from "@mui/material"
import { useState } from "react"
import { useForm } from "react-hook-form"
import type { Dispatch, SetStateAction, ReactNode } from "react"
import { QRCode } from "@/shared/ui/qr-code"

interface FormValues {
    firstName: string
    lastName: string
    patronymic: string
    group: string
}


interface Props {
    addButton: (setIsOpen: Dispatch<SetStateAction<boolean>>) => ReactNode
    onSubmit: (data: FormValues) => void
}

export const AddStudentForm = (props: Props) => {
    const { addButton, onSubmit } = props

    const [isOpen, setIsOpen] = useState(false)
    const {
        register,
        handleSubmit,
        formState: { errors }
    } = useForm<FormValues>()

    return (
        <>
            {addButton(setIsOpen)}
            {isOpen && (
                <Box
                    className="flex flex-col gap-4"
                    component="form"
                    sx={{
                        backgroundColor: "white",
                    }}
                    onSubmit={handleSubmit(onSubmit)}
                >
                    <QRCode value="1234567890" />
                </Box>
            )}
        </>
    )
}
export default AddStudentForm