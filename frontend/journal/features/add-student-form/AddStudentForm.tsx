import { Box, TextField } from "@mui/material"
import { useState } from "react"
import { useForm } from "react-hook-form"
import type { Dispatch, SetStateAction, ReactNode } from "react"

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
                    <TextField label="Имя" size="small" variant="outlined" {...register("firstName")} />
                    <TextField label="Фамилия" size="small" variant="outlined" {...register("lastName")} />
                    <TextField label="Отчество" size="small" variant="outlined" {...register("patronymic")} />
                    <TextField label="Группа" size="small" variant="outlined" {...register("group")} />
                </Box>
            )}
        </>
    )
}
export default AddStudentForm