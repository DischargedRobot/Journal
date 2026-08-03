"use client"

import Box from "@mui/material/Box"
import TextField from "@mui/material/TextField"
import { memo, useState } from "react"
import { useForm } from "react-hook-form"

const AddDisciplineForm = () => {
	const {
		handleSubmit,
		register,
		formState: { errors },
	} = useForm()

	const [shortName, setShortName] = useState<string>("")
	const [name, setName] = useState<string>("")
	const [isEditable, setIsEditable] = useState<boolean>(false)

	return (
		<Box>
			<form onSubmit={handleSubmit(() => {})}>
				<TextField
					label={"Название"}
					placeholder="Название"
					{...register("name", {
						required: "Название дисциплины обязательно",
					})}
					value={name}
					onChange={(e) => setName(e.currentTarget.value)}
				/>
				<TextField
					label={"Короткое название"}
					placeholder="Короткое название"
					{...register("shortName", {
						required: "Краткое название дисциплиын обязательно",
					})}
					value={
						shortName == "" || !isEditable
							? name.toLocaleUpperCase().split(" ").join("")
							: shortName
					}
					onChange={(e) => {
						setShortName(e.currentTarget.value)
						setIsEditable(true)
					}}
				/>
			</form>
		</Box>
	)
}

export default memo(AddDisciplineForm)
