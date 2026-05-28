import { DisciplineCard } from "@/entities/discipline"
import { TDiscipline } from "@/shared/model/discipline"
import List from "@mui/material/List"

interface Props {
	disciplines: TDiscipline[]
}

const DisciplineTable = ({ disciplines }: Props) => {
	return (
		<List>
			{disciplines.map((discipline) => (
				<DisciplineCard key={discipline.uuid} discipline={discipline} />
			))}
		</List>
	)
}

export default DisciplineTable
