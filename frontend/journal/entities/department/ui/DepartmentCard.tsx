"use client"

import { MoreToolsButton } from "@/shared/ui/more-tools-button"
import { TDepartment } from "@/shared/model/t-department"
import { TProfessor } from "@/shared/model/professor"
import Box from "@mui/material/Box"
import Chip from "@mui/material/Chip"
import { AddChip } from "@/shared/ui/add-chip"

interface Props {
	department: TDepartment
	professors: TProfessor[]
}

const DepartmentCard = ({ department, professors }: Props) => {
	return (
		<Box
			className="flex flex-col gap-2 p-4 border rounded-[20px] max-w-[300px] w-fit h-[211px] shadow-lg"
			sx={{
				bgcolor: "secondary.light",
				borderColor: "secondary.dark",
			}}
		>
			<div className="flex justify-between">
				<span className="text-lg  px-[10px] py-[5px] rounded-[20px] border">
					{department.shortName}
				</span>
				<span className="text-lg px-[10px] py-[5px] rounded-[20px] border">
					{department.name}
				</span>

				<MoreToolsButton
					items={[
						{
							key: "delete",
							label: "Удалить",
							sx: {
								color: "error.main",
							},
							onClick: () => {},
						},
						{
							key: "edit",
							label: "Редактировать",
							onClick: () => {},
						},
					]}
				/>
			</div>
			<Box
				className="overflow-y-auto flex flex-wrap content-start gap-2.5 p-2.5 w-full h-full rounded-[20px]"
				sx={{
					bgcolor: "secondary.main",
				}}
			>
				<AddChip label="Добавить преподавателя" />
				{professors.map((professor) => (
					<Chip
						key={professor.uuid}
						className="text-sm px-2.5 py-0.5 rounded-[20px] h-fit w-fit"
						title={`${professor.firstName} ${professor.lastName} ${professor.patronymic}`}
						label={`${professor.firstName} ${professor.lastName.slice(0, 1).toUpperCase()}. ${professor.patronymic ? professor.patronymic.slice(0, 1).toUpperCase() + "." : ""}`}
						sx={{
							backgroundColor: "secondary.light",
							"&:hover": {
								backgroundColor: "primary.main",
								color: "primary.contrastText",
							},
						}}
					/>
				))}
			</Box>
		</Box>
	)
}

export default DepartmentCard
