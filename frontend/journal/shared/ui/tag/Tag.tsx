import { Typography } from "@mui/material"
import Box from "@mui/material/Box"

interface Props {
	name: string
	color?: string
}

const Tag = ({ name, color = "lightgray" }: Props) => {
	return (
		<Box className="p-2 " style={{ backgroundColor: color }}>
			<Typography>{name}</Typography>
		</Box>
	)
}

export default Tag
