"use client"

import Box from "@mui/material/Box"
import Typography from "@mui/material/Typography"
import AddIcon from "@mui/icons-material/Add"
import { useState } from "react"
import TextField from "@mui/material/TextField"
import { useForm } from "react-hook-form"
import { TDepartment } from "@/shared/model/t-department/TDepartment"

interface Props {
	onClick: () => void
}

const AddDepartment = ({ onClick }: Props) => {
	const [isOpen, setIsOpen] = useState(false)

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<TDepartment>()

	const onSubmit = (data: TDepartment) => {
		console.log(data)
		setIsOpen(false)
		onClick()
	}

	return (
		<Box
			className="flex flex-col items-center justify-center gap-2 p-4 border rounded-[20px] w-[300px]  h-[211px] cursor-pointer"
			sx={{
				bgcolor: "secondary.light",
				borderColor: "contrastingSecondary.light",
				transition: "box-shadow 0.2s ease-in-out",
				"&:hover": {
					boxShadow: "0 4px 10px 0 var(--color-shadow)",
					"& .add-department-icon, & .add-department-title": {
						color: "primary.main",
					},
				},
			}}
			onClick={() => {
				setIsOpen(true)
				onClick()
			}}
		>
			{isOpen ? (
				<form onSubmit={handleSubmit(onSubmit)}>
					<TextField
						label="Название кафедры"
						variant="outlined"
						fullWidth
						required
						{...register("name", {
							required: "Название кафедры обязательно",
						})}
						error={!!errors.name}
						helperText={errors.name?.message ?? " "}
					/>
					<TextField
						label="Краткое название кафедры"
						variant="outlined"
						fullWidth
						required
						{...register("shortName", {
							required: "Название кафедры обязательно",
						})}
						error={!!errors.shortName}
						helperText={errors.shortName?.message ?? " "}
					/>
				</form>
			) : (
				<>
					<AddIcon
						className="add-department-icon"
						sx={{
							width: 64,
							height: 64,
							color: "contrastingSecondary.main",
						}}
					/>
					<Typography
						variant="body2"
						className="add-department-title title title_small px-[10px] py-[5px]"
						sx={{
							color: "contrastingSecondary.main",
						}}
					>
						Добавить кафедру
					</Typography>
				</>
			)}
		</Box>
	)
}

export default AddDepartment
