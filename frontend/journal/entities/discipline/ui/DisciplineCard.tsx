import { TDiscipline } from "@/shared/lib/discipline"
import Card from "@mui/material/Card"
import CardContent from "@mui/material/CardContent"
import CardHeader from "@mui/material/CardHeader"
import Typography from "@mui/material/Typography"
import { memo } from "react"
import DisciplineTag from "./DisciplineTag"

interface Props {
	discipline: Pick<TDiscipline, "name" | "type">
}

const DisciplineCard = ({ discipline }: Props) => {
	return (
		<Card>
			<CardHeader title={discipline.name} />
			<CardContent>
				<Typography variant="body1">{discipline.name}</Typography>
				<DisciplineTag disciplineType={discipline.type} />
			</CardContent>
		</Card>
	)
}

export default memo(DisciplineCard)
