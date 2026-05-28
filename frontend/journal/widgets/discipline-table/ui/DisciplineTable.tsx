import { DisciplineCard } from "@/entities/discipline"
import { TDiscipline } from "@/shared/model/discipline"
import Box from "@mui/material/Box"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import Typography from "@mui/material/Typography"

interface Props {
	disciplines: TDiscipline[]
}

const DisciplineTable = ({ disciplines }: Props) => {

	const grouped = disciplines.reduce<Record<string, TDiscipline[]>>(
		(acc, discipline) => {
			if (!acc[discipline.name]) {
				acc[discipline.name] = []
			}

			acc[discipline.name].push(discipline)

			return acc
		},
		{},
	)

	return (
		<Box sx={{ display: "flex", gap: 4 }}>
			{Object.entries(grouped).map(([name, disciplines]) => (
				<div key={name} className="flex flex-col items-center gap-1">
					<Typography
						className="font-bold"
						sx={{ color: "primary.dark" }}
						align="center"
						variant="h6"
					>
						{name}
					</Typography>
					<List className="flex flex-col gap-5">
						{disciplines.map((discipline) => (
							<ListItem key={discipline.uuid} disablePadding>
								<DisciplineCard
									key={discipline.uuid}
									discipline={discipline}
								/>
							</ListItem>
						))}
					</List>
				</div>
			))}
		</Box>
	)
}

export default DisciplineTable
