"use client"

import Box from "@mui/material/Box"
import Typography from "@mui/material/Typography"
import AddIcon from "@mui/icons-material/Add"

interface Props {
	onClick: () => void
}

const AddDepartment = ({ onClick }: Props) => {
	return (
		<Box
			className="flex flex-col items-center justify-center gap-2 p-4 border rounded-[20px] w-[300px] h-[211px]"
			sx={{
				bgcolor: "secondary.light",
				borderColor: "contrastingSecondary.light",
				transition: "box-shadow 0.2s ease-in-out",
				"&:hover": {
					boxShadow: "0 4px 10px 0 var(--color-shadow)",
					"& *": {
						color: "primary.main",
					},
				},
			}}
			onClick={onClick}
		>
			<AddIcon
				sx={{
					width: 64,
					height: 64,
					color: "contrastingSecondary.main",
				}}
			/>
			<Typography
				variant="body2"
				className="title title_small px-[10px] py-[5px]"
				sx={{
					color: "contrastingSecondary.main",
				}}
			>
				Добавить кафедру
			</Typography>
		</Box>
	)
}

export default AddDepartment
