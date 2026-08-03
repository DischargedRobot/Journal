import { Typography } from "@mui/material"
import Box from "@mui/material/Box"
import { grey } from "@mui/material/colors"

export type TagColor = { bg: string; text: string }

export interface Props {
	name: string
	color?: TagColor
	icon?: React.ReactNode
}

const Tag = ({ name, color = { bg: grey[300], text: grey[700] }, icon }: Props) => {
	return (
		<Box
			className="flex items-center gap-1 px-2 py-1 rounded-2xl w-fit"
			sx={{ backgroundColor: color.bg }}
		>
			{icon}
			<Typography
				component="span"
				className="text-sm whitespace-nowrap"
				sx={{ color: color.text }}
			>
				{name}
			</Typography>
		</Box>
	)
}

export default Tag
