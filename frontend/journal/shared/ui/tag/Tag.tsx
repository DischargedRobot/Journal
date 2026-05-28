import { Typography } from "@mui/material"
import Box from "@mui/material/Box"
import { grey } from "@mui/material/colors"

export type TagColor = { bg: string; text: string }

export interface Props {
	name: string
	color?: TagColor
}

const Tag = ({ name, color = { bg: grey[300], text: grey[700] } }: Props) => {
	return (
		<Box
			className="px-2 py-0.5 rounded-2xl w-fit"
			sx={{ backgroundColor: color.bg }}
		>
			<Typography className="text-sm" sx={{ color: color.text }}>
				{name}
			</Typography>
		</Box>
	)
}

export default Tag
