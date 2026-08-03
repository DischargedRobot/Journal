import { TDepartment } from "@/shared/model/t-department"
import DepartmentCard from "../../../entities/department/ui/DepartmentCard"
import { TProfessor } from "@/shared/model/professor"
import Box from "@mui/material/Box"
import { Typography } from "@mui/material"
import AddDepartment from "@/features/add-department/ui/AddDepartment"

interface Props {
	items: {
		department: TDepartment
		professors: TProfessor[]
	}[]
	title: string
}
const DepartmentsList = ({ items, title }: Props) => {
	return (
		<Box className="flex flex-col w-full h-fit rounded-[20px] overflow-hidden">
			<Typography
				className="title title_small w-full"
				variant="body2"
				sx={{
					paddingX: "15px",
					paddingY: "10px",
					backgroundColor: "primary.main",
					color: "primary.contrastText",
				}}
			>
				{title}
			</Typography>
			<Box
				className="flex flex-wrap gap-2.5 p-2.5 w-full"
				sx={{
					backgroundColor: "secondary.light",
				}}
			>
				<AddDepartment
					onClick={() => {
						console.log("add department")
					}}
				/>
				{items.map(({ department, professors }) => (
					<DepartmentCard
						key={department.uuid}
						department={department}
						professors={professors}
					/>
				))}
			</Box>
		</Box>
	)
}

export default DepartmentsList
