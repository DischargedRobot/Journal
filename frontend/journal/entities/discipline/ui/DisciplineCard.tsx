import { TDiscipline } from "@/shared/model/discipline"
import Card from "@mui/material/Card"
import CardContent from "@mui/material/CardContent"
import CardHeader from "@mui/material/CardHeader"
import { memo } from "react"
import DisciplineTag from "./DisciplineTag"
import Typography from "@mui/material/Typography"

interface Props {
	discipline: Pick<TDiscipline, "name" | "type" | "shortName">
	professorName?: string
	onClick?: () => void
}

const DisciplineCard = ({ discipline, professorName, onClick }: Props) => {
	return (
		<Card
			className="relative rounded-xl border-b-4 w-full transition-all duration-200 easy-in-out hover:cursor-pointer hover:scale-[1.1]"
			sx={{
				bgcolor: "secondary.light",
				borderColor: "grey.300",
				"&:hover": {
					boxShadow: 5,
				}
			}}
			onClick={onClick}
		>
			<CardHeader
				className="flex items-start h-20 wrap-anywhere"
				variant="body2"
				disableTypography
				title={discipline.name.length > 30 ? discipline.shortName : discipline.name}
			/>
			<CardContent>
				<Typography variant="body2">{professorName}</Typography>
				<DisciplineTag disciplineType={discipline.type} />
			</CardContent>
		</Card>
	)
}

export default memo(DisciplineCard)
